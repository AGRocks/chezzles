﻿using AutoMapper;
using chezzles.engine.Core.Game;
using chezzles.engine.Pieces.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgn = ilf.pgn.Data;

namespace chezzles.engine.MapperSetup
{
    public class PgnGameMappingProfile : Profile
    {
        protected override void Configure()
        {
            this.CreateMap<Pgn.Piece, Core.Piece>()
                .ConvertUsing(new PieceTypeConverter());
            this.CreateMap<Pgn.PieceType, Core.PieceType>();

            this.CreateMap<Pgn.Square, Core.Square>()
                .ForMember(d => d.XPosition, opt => opt.MapFrom(s => s.File))
                .ForMember(d => d.YPosition, opt => opt.MapFrom(s => s.Rank));

            this.CreateMap<Pgn.Move, Core.Game.Move>();

            this.CreateMap<Pgn.BoardSetup, Core.Board>()
                .AfterMap((s, board) =>
                {
                    for (int i = 0; i <= 7; i++)
                        for (int j = 0; j <= 7; j++)
                        {
                            var piece = Mapper.Map<Core.Piece>(s[i, j]);
                            if (piece != null)
                            {
                                piece.Board = board;
                                board.PutPiece(new Core.Square(i + 1, j + 1), piece);
                            }
                        }
                });

            CreateMap<Pgn.MoveText.MoveTextEntryList, List<Move>>();

            this.CreateMap<Pgn.Game, Core.Game.Game>()
                .ForMember(d => d.Board, opt => opt.MapFrom(x => x.BoardSetup))
                .ForMember(d => d.Moves, opt => opt.MapFrom(x => x.MoveText));

            this.CreateMap<Pgn.Database, IEnumerable<Core.Game.Game>>()
                .ConstructUsing(x => x.Games.Select(g => Mapper.Map<Game>(g)));

            base.Configure();
        }   
    }
}
